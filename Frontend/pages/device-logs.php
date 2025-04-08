<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

require_login();

function decode_jwt_payload($jwt) {
  $parts = explode('.', $jwt);
  if (count($parts) !== 3) return null;
  $payload = $parts[1];
  $payload = str_replace(['-', '_'], ['+', '/'], $payload);
  $payload .= str_repeat('=', (4 - strlen($payload) % 4) % 4);
  return json_decode(base64_decode($payload), true);
}

$decoded = decode_jwt_payload($_SESSION['user_token']);
$userId = $decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? null;
if (!$userId) die("User ID not found in token.");

// Get query params
$selectedDevice = $_GET['deviceId'] ?? null;
$statusFilter = $_GET['status'] ?? null;
$page = isset($_GET['page']) ? max(1, (int) $_GET['page']) : 1;
$logsPerPage = 10;

// Get device IDs
$deviceIds = [];
$device_api_url = $baseAPI . "User_Device/device-ids/$userId";
$ch = curl_init($device_api_url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, ["Authorization: Bearer " . $_SESSION['user_token']]);
$response = curl_exec($ch);
curl_close($ch);
if ($response) {
  $deviceIds = json_decode($response, true);
}

// Fetch logs
$logs = [];
if ($selectedDevice || count($deviceIds)) {
  $targetDevices = $selectedDevice ? [$selectedDevice] : $deviceIds;

  foreach ($targetDevices as $deviceId) {
    $logUrl = $baseAPI . "Logs/device/" . urlencode($deviceId);
    $ch = curl_init($logUrl);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_HTTPHEADER, ["Authorization: Bearer " . $_SESSION['user_token']]);
    $logResponse = curl_exec($ch);
    curl_close($ch);

    if ($logResponse) {
      $deviceLogs = json_decode($logResponse, true);
      if (is_array($deviceLogs)) {
        foreach ($deviceLogs as $log) {
          $logs[] = $log;
        }
      }
    }
  }
}

// Filter by status if selected
if ($statusFilter === "triggered") {
  $logs = array_filter($logs, fn($log) => $log['isTriggered'] ?? false);
} elseif ($statusFilter === "normal") {
  $logs = array_filter($logs, fn($log) => !($log['isTriggered'] ?? false));
}

// Sort logs (newest first)
usort($logs, function ($a, $b) {
  return strtotime($b['createdAt'] ?? 'now') <=> strtotime($a['createdAt'] ?? 'now');
});

// Pagination
$totalLogs = count($logs);
$totalPages = ceil($totalLogs / $logsPerPage);
$start = ($page - 1) * $logsPerPage;
$logsToDisplay = array_slice($logs, $start, $logsPerPage);
?>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sentinel – Device Logs</title>
</head>
<body class="<?= $defaultBackgroundColor ?>">

<?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

<section>
  <div class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?> w-full">
      <h2 class="<?= $sectionHeading ?>">Device Logs</h2>

      <?php if (!empty($deviceIds)) : ?>
      <form method="GET" class="mb-6 text-white grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Filter by device -->
        <div>
          <label for="deviceId" class="block text-sm font-medium mb-2">Filter by device:</label>
          <select name="deviceId" id="deviceId" onchange="this.form.submit()"
            class="w-full p-2 rounded bg-gray-700 text-white border border-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-400">
            <option value="">All Devices</option>
            <?php foreach ($deviceIds as $id): ?>
              <option value="<?= $id ?>" <?= $selectedDevice === $id ? 'selected' : '' ?>>
                <?= htmlspecialchars($id) ?>
              </option>
            <?php endforeach; ?>
          </select>
        </div>

        <!-- Filter by status -->
        <div>
          <label for="status" class="block text-sm font-medium mb-2">Filter by status:</label>
          <select name="status" id="status" onchange="this.form.submit()"
            class="w-full p-2 rounded bg-gray-700 text-white border border-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-400">
            <option value="">All</option>
            <option value="triggered" <?= $statusFilter === "triggered" ? 'selected' : '' ?>>🚨 Triggered</option>
            <option value="normal" <?= $statusFilter === "normal" ? 'selected' : '' ?>>✅ Normal</option>
          </select>
        </div>
      </form>
      <?php endif; ?>

      <?php if (empty($logsToDisplay)) : ?>
        <p class="<?= $sectionParagraph ?>">No logs found for your filters.</p>
      <?php else : ?>
        <ul class="space-y-4 text-white text-sm">
          <?php foreach ($logsToDisplay as $log): ?>
          <li class="bg-gray-800 p-4 rounded shadow">
            <p><strong>Device ID:</strong> <?= htmlspecialchars($log['deviceId'] ?? 'Unknown') ?></p>
            <p><strong>Start Date:</strong>
              <?= isset($log['date']) ? sprintf('%04d-%02d-%02d', $log['date']['year'], $log['date']['month'], $log['date']['day']) : 'Unknown' ?>
            </p>
            <p><strong>End Date:</strong>
              <?= isset($log['endDate']) ? sprintf('%04d-%02d-%02d', $log['endDate']['year'], $log['endDate']['month'], $log['endDate']['day']) : 'Unknown' ?>
            </p>
            <p><strong>Armed Time:</strong>
              <?= isset($log['armedTime']) ? sprintf('%02d:%02d', $log['armedTime']['hour'], $log['armedTime']['minute']) : 'Unknown' ?>
            </p>
            <p><strong>Disarmed Time:</strong>
              <?= isset($log['disarmedTime']) ? sprintf('%02d:%02d', $log['disarmedTime']['hour'], $log['disarmedTime']['minute']) : 'Unknown' ?>
            </p>
            <p><strong>Status:</strong>
              <?= isset($log['isTriggered']) && $log['isTriggered'] ? '🚨 Triggered' : '✅ Normal' ?>
            </p>
            <p><strong>Triggered Time:</strong>
              <?= isset($log['triggeredTime']) ? sprintf('%02d:%02d', $log['triggeredTime']['hour'], $log['triggeredTime']['minute']) : 'Unknown' ?>
            </p>
          </li>
          <?php endforeach; ?>
        </ul>

        <!-- Pagination -->
        <?php if ($totalPages > 1): ?>
        <div class="mt-6 flex justify-center gap-2">
          <?php for ($i = 1; $i <= $totalPages; $i++): ?>
          <a href="?<?= http_build_query(array_merge($_GET, ['page' => $i])) ?>"
            class="px-3 py-1 rounded border text-sm <?= $i == $page ? 'bg-blue-600 text-white' : 'bg-white text-gray-700' ?>">
            <?= $i ?>
          </a>
          <?php endfor; ?>
        </div>
        <?php endif; ?>
      <?php endif; ?>
    </div>
  </div>
</section>

<?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>
</html>

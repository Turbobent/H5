<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
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

// Fetch devices
$devices = [];
$device_api_url = $baseAPI . "User_Device/device-ids/$userId";

$ch = curl_init($device_api_url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
  "Authorization: Bearer " . $_SESSION['user_token']
]);
$response = curl_exec($ch);
$http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);
if ($http_code === 200) {
  $devices = json_decode($response, true);
}

// Handle Arm/Disarm
if ($_SERVER["REQUEST_METHOD"] === "POST" && isset($_POST['action'])) {
  $newStatus = $_POST['action'] === 'arm';
  foreach ($devices as $deviceId) {
    $updateUrl = $baseAPI . "Devices/UpdateStatus/" . urlencode($deviceId);
    $payload = json_encode(["status" => $newStatus]);

    $ch = curl_init($updateUrl);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "PUT");
    curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
      "Content-Type: application/json",
      "Authorization: Bearer " . $_SESSION['user_token']
    ]);
    curl_exec($ch);
    curl_close($ch);
  }

  header("Location: " . $baseURL . "dashboard");
  exit;
}

// Handle delete device
if ($_SERVER["REQUEST_METHOD"] === "POST" && isset($_POST['deleteDeviceId'])) {
  $deviceIdToDelete = $_POST['deleteDeviceId'];
  $deleteUrl = $baseAPI . "Devices/" . urlencode($deviceIdToDelete);

  $ch = curl_init($deleteUrl);
  curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "DELETE");
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
  curl_setopt($ch, CURLOPT_HTTPHEADER, [
    "Authorization: Bearer " . $_SESSION['user_token']
  ]);
  curl_exec($ch);
  curl_close($ch);

  header("Location: " . $baseURL . "dashboard");
  exit;
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sentinel - Dashboard</title>
</head>
<body class="<?= $defaultBackgroundColor ?>">

  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <section>
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <div class="flex flex-col items-center justify-center gap-4 <?= $sectionBox ?>">

        <h2 class="<?= $sectionHeading ?>">Welcome to your dashboard</h2>

        <?php if (empty($devices)) : ?>
          <p class="<?= $sectionParagraph ?>">You don't have any devices registered yet.</p>
          <a href="<?= $baseURL ?>register-device" class="<?= $formButton ?>">Register Device</a>
        <?php else : ?>
          <div class="w-full flex justify-end">
            <a href="<?= $baseURL ?>register-device" class="mb-2 bg-blue-600 hover:bg-blue-700 text-white text-sm px-4 py-2 rounded">
              + Register New Device
            </a>
          </div>

          <h3 class="text-white font-semibold text-lg">Your Devices:</h3>
          <ul class="text-white space-y-4 text-sm text-left w-full">
            <?php foreach ($devices as $deviceId): ?>
              <?php
                $deviceDetailUrl = $baseAPI . "Devices/" . urlencode($deviceId);
                $ch = curl_init($deviceDetailUrl);
                curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
                curl_setopt($ch, CURLOPT_HTTPHEADER, [
                  "Authorization: Bearer " . $_SESSION['user_token']
                ]);
                $detailResponse = curl_exec($ch);
                $detailCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
                curl_close($ch);

                if ($detailCode === 200) {
                  $device = json_decode($detailResponse, true);
                  $deviceName = htmlspecialchars($device['name'] ?? 'Unnamed Device');
                  $deviceStatus = isset($device['status']) && $device['status'] ? '🔐 Armed' : '🔓 Disarmed';
              ?>
              <li class="bg-gray-800 p-4 rounded shadow">
                <p><strong>Name:</strong> <?= $deviceName ?></p>
                <p><strong>Status:</strong> <?= $deviceStatus ?></p>
                <p class="text-gray-400 text-xs">ID: <?= htmlspecialchars($device['deviceId']) ?></p>
                <a href="<?= $baseURL ?>edit-device?deviceId=<?= urlencode($device['deviceId']) ?>"
                  class="inline-block mt-2 bg-blue-600 hover:bg-blue-700 text-white text-xs px-4 py-1 rounded">
                  Edit name
                </a>
                <button onclick="toggleModal(true, '<?= htmlspecialchars($device['deviceId']) ?>')"
                  class="inline-block mt-2 bg-red-600 hover:bg-red-700 text-white text-xs px-4 py-1 rounded ml-2">
                  Delete Device
                </button>
              </li>
              <?php } else { ?>
              <li class="text-red-400">Failed to load device info for ID: <?= htmlspecialchars($deviceId) ?></li>
              <?php } ?>
            <?php endforeach; ?>
          </ul>

          <form method="POST" class="flex gap-4 mt-6">
            <button type="submit" name="action" value="arm"
              class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded">Arm</button>
            <button type="submit" name="action" value="disarm"
              class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded">Disarm</button>
          </form>

          <a href="<?= $baseURL ?>device-logs"
            class="mt-6 inline-block bg-gray-700 hover:bg-gray-800 text-white px-4 py-2 rounded text-sm">
            📄 View Device Logs
          </a>
        <?php endif; ?>

      </div>
    </div>
  </section>

  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>

  <!-- Delete Modal -->
  <div id="deleteModal" class="fixed inset-0 z-40 bg-black/30 backdrop-blur-sm flex items-center justify-center hidden">
    <div id="modalBox" class="bg-white text-black rounded-lg shadow-lg p-6 max-w-sm w-full relative">
      <h3 class="text-lg font-semibold mb-4">Delete Device</h3>
      <p>Are you sure you want to delete this device? This action cannot be undone.</p>
      <div class="mt-6 flex justify-end gap-3">
        <button onclick="toggleModal(false)"
          class="px-4 py-2 bg-gray-300 text-gray-800 rounded hover:bg-gray-400">Cancel</button>
        <form method="POST" id="deleteForm">
          <input type="hidden" name="deleteDeviceId" id="deleteDeviceId">
          <button type="submit" class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700">Yes delete</button>
        </form>
      </div>
    </div>
  </div>

  <script>
    function toggleModal(show, deviceId = null) {
      const modal = document.getElementById('deleteModal');
      const input = document.getElementById('deleteDeviceId');

      if (show && deviceId) {
        input.value = deviceId;
        modal.classList.remove('hidden');
      } else {
        modal.classList.add('hidden');
        input.value = '';
      }
    }

    // Close modal if clicking directly on backdrop (not modal content)
    document.getElementById('deleteModal').addEventListener('click', function (e) {
      if (e.target.id === 'deleteModal') {
        toggleModal(false);
      }
    });

    // ESC closes modal
    window.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') {
        toggleModal(false);
      }
    });
  </script>

</body>
</html>

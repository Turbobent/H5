<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

require_login();

$deviceId = $_GET['deviceId'] ?? null;

if (!$deviceId) {
  die("Device ID is missing.");
}

$device = null;
$error_message = "";
$success_message = "";

// GET device info (same as before)
$api_url = $baseAPI . "Devices/" . urlencode($deviceId);

$ch = curl_init($api_url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
  "Authorization: Bearer " . $_SESSION['user_token']
]);
$response = curl_exec($ch);
$http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($http_code === 200) {
  $device = json_decode($response, true);
} elseif ($http_code === 403 || $http_code === 500) {
  $responseData = json_decode($response, true);
  $error_message = $responseData['message'] ?? "You do not have permission to access this device.";
} else {
  $error_message = "Failed to load device information. Code: $http_code";
}

// Handle form submission
if ($_SERVER["REQUEST_METHOD"] === "POST") {
  $updated_name = $_POST['name'];

  $payload = json_encode([
    "newName" => $updated_name
  ]);

  $ch = curl_init($baseAPI . "Devices/UpdateName/" . urlencode($deviceId));
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
  curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "PUT");
  curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);
  curl_setopt($ch, CURLOPT_HTTPHEADER, [
    "Content-Type: application/json",
    "Authorization: Bearer " . $_SESSION['user_token']
  ]);

  $updateResponse = curl_exec($ch);
  $updateCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
  $curl_error = curl_error($ch);
  curl_close($ch);

  if ($updateCode === 403 || $updateCode === 500) {
    $error_message = "test";
  }

  if ($updateCode === 200 || $updateCode === 204) {
    $success_message = "Device name updated successfully!";
    // Optional: Refresh device name
    $device['name'] = $updated_name;
  } else {
    $error_message = "Failed to load device information. Code: $http_code";
  }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Edit Device – Sentinel</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <section class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?>">
      <h2 class="<?= $sectionHeading ?>">Edit Device</h2>

      <?php if ($success_message): ?>
      <p class="text-green-500 mb-4"><?= htmlspecialchars($success_message) ?></p>
      <?php elseif ($error_message): ?>
      <p class="text-red-500 mb-4"><?= htmlspecialchars($error_message) ?></p>
      <?php endif; ?>

      <?php if ($device): ?>
      <form method="POST" class="space-y-6">
        <div>
          <label for="name" class="<?= $formLabel ?>">Device Name</label>
          <input type="text" name="name" id="name" class="<?= $formInput ?>"
            value="<?= htmlspecialchars($device['name']) ?>" required>
        </div>

        <button type="submit" class="<?= $formButton ?>">Update Device</button>
      </form>
      <?php endif; ?>
    </div>
  </section>

  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
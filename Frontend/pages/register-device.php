<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect if not logged in
require_login();

// Init messages
$success_message = "";
$error_message = "";

// Handle form submit
if ($_SERVER["REQUEST_METHOD"] === "POST") {
  $name = $_POST['name'];
  $deviceId = $_POST['deviceId'];

  $data = json_encode([
    "name" => $name,
    "deviceId" => $deviceId,
  ]);

  $ch = curl_init($baseAPI . "Devices");
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
  curl_setopt($ch, CURLOPT_POST, true);
  curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
  curl_setopt($ch, CURLOPT_HTTPHEADER, [
    "Content-Type: application/json",
    "Authorization: Bearer " . $_SESSION['user_token']
  ]);

  $response = curl_exec($ch);
  $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
  $curl_error = curl_error($ch);
  curl_close($ch);

  if ($http_code === 200) {
    $success_message = "Device successfully registered!";
  } else {
    $error_message = $curl_error ?: "Something went wrong. Code: $http_code";
  }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Register Device – Sentinel</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Main -->
  <section class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?>">
      <h2 class="<?= $sectionHeading ?>">Register a New Device</h2>

      <?php if ($success_message): ?>
        <p class="text-green-500 text-sm mb-4"><?= htmlspecialchars($success_message) ?></p>
      <?php elseif ($error_message): ?>
        <p class="text-red-500 text-sm mb-4"><?= htmlspecialchars($error_message) ?></p>
      <?php endif; ?>

      <form method="POST" class="space-y-6">
        <div>
          <label for="name" class="<?= $formLabel ?>">Device Name</label>
          <input type="text" name="name" id="name" class="<?= $formInput ?>" required placeholder="Living Room Alarm">
        </div>
        <div>
          <label for="deviceId" class="<?= $formLabel ?>">Device ID</label>
          <input type="text" name="deviceId" id="deviceId" class="<?= $formInput ?>" required placeholder="ABC123">
        </div>
        <button type="submit" class="<?= $formButton ?>">Register Device</button>
      </form>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>

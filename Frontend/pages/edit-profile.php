<?php
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect if not logged in
require_login();

// Decode token to get user ID
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

if (!$userId) {
  die("Unable to retrieve user ID from token.");
}

// Variables
$success_message = "";
$error_message = "";
$email = "";
$username = "";

// GET user data
$api_get_url = "https://sentinal-api.mercantec.tech/api/Users/$userId";

$ch = curl_init($api_get_url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
    "Authorization: Bearer " . $_SESSION['user_token']
]);

$response = curl_exec($ch);
$http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($http_code === 200) {
    $userData = json_decode($response, true);
    $email = $userData['email'] ?? '';
    $username = $userData['username'] ?? '';
} else {
    $error_message = "Unable to fetch user data. Code: $http_code";
}

// Update the profile
if ($_SERVER["REQUEST_METHOD"] === "POST") {
  $email = $_POST['email'];
  $username = $_POST['username'];
  $password = $_POST['password'];
  $confirmPassword = $_POST['confirm_password'];

  // Validate password confirmation (only if user entered a password)
  if (!empty($password) && $password !== $confirmPassword) {
      $error_message = "Passwords do not match.";
  } else {
    $payload = [
        "email" => $email,
        "username" => $username,
        "password" => $password
    ];

    $api_put_url = "https://sentinal-api.mercantec.tech/api/Users/$userId";

    $ch = curl_init($api_put_url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "PUT");
    curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($payload));
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "Content-Type: application/json",
        "Authorization: Bearer " . $_SESSION['user_token']
    ]);

    $response = curl_exec($ch);
    $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $curl_error = curl_error($ch);
    curl_close($ch);

    if ($http_code === 200 || $http_code === 204) {
        $success_message = "Profile updated successfully!";
    } else {
        $error_message = $curl_error ?: "Failed to update profile. Code: $http_code";
    }
  }
}

?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sentinel - Edit Profile</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">

  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Main -->
  <section class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?>">
      <h2 class="<?= $sectionHeading ?>">Edit Profile</h2>
      <p class="<?= $sectionParagraph ?>">Update your account details. Leave password fields empty to keep your current
        password.</p>

      <!-- Flash messages -->
      <?php if (!empty($success_message)) : ?>
      <p class="text-green-500 text-center text-sm mb-4"><?= htmlspecialchars($success_message) ?></p>
      <?php endif; ?>
      <?php if (!empty($error_message)) : ?>
      <p class="text-red-500 text-center text-sm mb-4"><?= htmlspecialchars($error_message) ?></p>
      <?php endif; ?>

      <!-- Form -->
      <form method="POST" class="space-y-6">
        <div>
          <label for="email" class="<?= $formLabel ?>">Email</label>
          <input type="email" name="email" id="email" value="<?= htmlspecialchars($email) ?>" class="<?= $formInput ?>"
            required>
        </div>

        <div>
          <label for="username" class="<?= $formLabel ?>">Username</label>
          <input type="text" name="username" id="username" value="<?= htmlspecialchars($username) ?>"
            class="<?= $formInput ?>" required>
        </div>

        <div>
          <label for="password" class="<?= $formLabel ?>">New Password (optional)</label>
          <input type="password" name="password" id="password" placeholder="Enter new password"
            class="<?= $formInput ?>">
        </div>

        <div>
          <label for="confirm_password" class="<?= $formLabel ?>">Confirm New Password</label>
          <input type="password" name="confirm_password" id="confirm_password" placeholder="Re-enter new password"
            class="<?= $formInput ?>">
        </div>

        <button type="submit" class="<?= $formButton ?>">Save Changes</button>
      </form>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
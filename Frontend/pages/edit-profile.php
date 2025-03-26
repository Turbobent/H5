<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");

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
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sentinel - Edit Profile</title>
</head>

<body class="bg-[#6CD9D9]">

  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Main -->
  <section>
    <div class="flex flex-col items-center justify-center px-6 py-8 mx-auto md:min-h-[calc(100vh-140px)] lg:py-0">
      <h2 class="text-2xl font-bold mb-4 text-gray-800">Edit Profile</h2>

      <?php if (!empty($success_message)) : ?>
      <p class="text-green-600 text-sm mb-4"><?= htmlspecialchars($success_message) ?></p>
      <?php endif; ?>

      <?php if (!empty($error_message)) : ?>
      <p class="text-red-600 text-sm mb-4"><?= htmlspecialchars($error_message) ?></p>
      <?php endif; ?>

      <!-- Edit Profile Form -->
      <form method="POST" class="space-y-4 w-full max-w-md bg-white p-6 rounded shadow">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
          <input type="email" name="email" id="email" value="<?= htmlspecialchars($email) ?>"
            class="w-full rounded border p-2" required />
        </div>

        <div>
          <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
          <input type="text" name="username" id="username" value="<?= htmlspecialchars($username) ?>"
            class="w-full rounded border p-2" required />
        </div>

        <div>
          <label for="password" class="block text-sm font-medium text-gray-700">New Password (optional)</label>
          <input type="password" name="password" id="password" class="w-full rounded border p-2"
            placeholder="Enter new password" />
        </div>

        <button type="submit" class="w-full bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
          Save Changes
        </button>
      </form>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
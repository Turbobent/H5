<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect logged-in users
if (isset($_SESSION['user_token'])) {
    header("Location: " . $baseURL . "dashboard");
    exit;
}

// Initialize error message
$error_message = "";

// Handle form submission
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $username = $_POST['emailOrUsername']; // Use 'username' to match API
    $password = $_POST['password'];

    // API URL
    $api_url = $baseAPI . "Users/login";

    // Prepare API request data
    $data = json_encode([
        "username" => $username,
        "password" => $password
    ]);

    // Set up cURL request
    $ch = curl_init($api_url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POST, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        "Content-Type: application/json"
    ]);

    // Execute request & get response
    $response = curl_exec($ch);
    $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $curl_error = curl_error($ch);
    curl_close($ch);

    // Check if API response is valid JSON
    $result = json_decode($response, true);

    if ($http_code == 200 && isset($result['token'])) {
        // Login successful, store token in session
        $_SESSION['user_token'] = $result['token'];
        header("Location: " . $baseURL . "dashboard");
        exit;
    } else {
        // Handle API errors
        if ($curl_error) {
            $error_message = "Connection error. Please try again. " . $http_code;
        } elseif (isset($result['message'])) {
            $error_message = $result['message'];
        } else {
            $error_message = "Invalid username or password. " . $http_code;
        }
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Sentinel - Login</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Main -->
  <section>
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <!-- Images and Sentinel text -->
      <div class="flex items-center mb-6 text-2xl font-semibold text-gray-900 dark:text-white">
        <img class="w-16 h-16 mr-2" src="<?= $baseURL; ?>images/Sentinel-logo.png" alt="logo">
        Sentinel
      </div>

      <!-- Login form -->
      <div
        class="w-full bg-white rounded-lg shadow dark:border sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
        <div class="p-6 space-y-4 md:space-y-6 sm:p-8">
          <h1 class="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
            Sign in to your account
          </h1>

          <!-- Error Message -->
          <?php if (!empty($error_message)) : ?>
          <p class="text-red-500 text-sm"><?= htmlspecialchars($error_message); ?></p>
          <?php endif; ?>

          <form class="space-y-4 md:space-y-6" method="POST">
            <div>
              <label for="emailOrUsername" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                Your username
              </label>
              <input type="text" name="emailOrUsername" id="emailOrUsername"
                class="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                placeholder="Enter your username" required>
            </div>

            <div>
              <label for="password"
                class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Password</label>
              <input type="password" name="password" id="password" placeholder="••••••••"
                class="bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                required>
            </div>

            <button type="submit"
              class="w-full text-white bg-blue-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800">
              Login
            </button>

            <p class="text-sm font-light text-gray-500 dark:text-gray-400">
              Don’t have an account yet?
              <a href="<?= $baseURL ?>signup"
                class="font-medium text-primary-600 hover:underline dark:text-primary-500">
                Sign up
              </a>
            </p>
          </form>
        </div>
      </div>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
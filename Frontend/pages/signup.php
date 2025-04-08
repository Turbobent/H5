<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect logged-in users
if (isset($_SESSION['user_token'])) {
    header("Location: " . $baseURL . "dashboard");
    exit;
}

// Initialize variables
$error_message = "";
$email = "";
$username = "";

// Handle form submission
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Retain input values
    $email = $_POST['email'];
    $username = $_POST['username'];
    $password = $_POST['password'];
    $confirmPassword = $_POST['confirm-password'];

    // Validate password confirmation
    if ($password !== $confirmPassword) {
        $error_message = "Passwords do not match.";
    } else {
        // API URL
        $api_url = $baseAPI . "Users/signup";

        // Prepare API request data
        $data = json_encode([
            "email" => $email,
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

        if ($http_code == 201 && isset($result['message'])) {
            // Signup successful, clear input and redirect to login
            header("Location: " . $baseURL . "pages/login.php?signup=success");
            exit;
        } else {
            // Handle API errors
            if ($curl_error) {
                $error_message = "Connection error. Please try again.";
            } elseif (isset($result['message'])) {
                $error_message = $result['message'];
            } else {
                $error_message = "Signup failed. Please try again.";
            }
        }
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Sentinel - Signup</title>
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

      <!-- Signup form -->
      <div
        class="w-full bg-white rounded-lg shadow dark:border sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
        <div class="p-6 space-y-4 md:space-y-6 sm:p-8">
          <h1 class="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
            Create an account
          </h1>

          <!-- Error Message -->
          <?php if (!empty($error_message)) : ?>
          <p class="text-red-500 text-sm"><?= htmlspecialchars($error_message); ?></p>
          <?php endif; ?>

          <form class="space-y-4 md:space-y-6" method="POST">
            <!-- Email -->
            <div>
              <label for="email" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Email</label>
              <input type="email" name="email" id="email"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                placeholder="name@company.com" required value="<?= htmlspecialchars($email); ?>">
            </div>
            <!-- Username -->
            <div>
              <label for="username"
                class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Username</label>
              <input type="text" name="username" id="username"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                placeholder="Username" required value="<?= htmlspecialchars($username); ?>">
            </div>
            <!-- Password -->
            <div>
              <label for="password"
                class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Password</label>
              <input type="password" name="password" id="password" placeholder="••••••••"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                required>
            </div>
            <!-- Confirm password -->
            <div>
              <label for="confirm-password" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Confirm
                password</label>
              <input type="password" name="confirm-password" id="confirm-password" placeholder="••••••••"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                required>
            </div>
            <!-- Submit btn -->
            <button type="submit"
              class="w-full text-white bg-blue-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800">
              Create an account
            </button>

            <p class="text-sm font-light text-gray-500 dark:text-gray-400">
              Already have an account?
              <a href="<?= $baseURL ?>login" class="font-medium text-primary-600 hover:underline dark:text-primary-500">
                Login here
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
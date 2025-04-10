<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect if already logged in
if (isset($_SESSION['user_token'])) {
    header("Location: " . $baseURL . "dashboard");
    exit;
}

// Initialize values
$error_message = "";
$email = "";
$username = "";

// Handle form submit
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $email = $_POST['email'];
    $username = $_POST['username'];
    $password = $_POST['password'];
    $confirmPassword = $_POST['confirm-password'];

    // Regex rules - match backend
    $validateUsername = '/^[a-zA-Z0-9]{5,15}$/';
    $validateEmail = '/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/';
    $validatePassword = '/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/';

    // Validate fields
    if (!preg_match($validateEmail, $email)) {
        $error_message = "Please enter a valid email address.";
    } elseif (!preg_match($validateUsername, $username)) {
        $error_message = "Username must be 5–15 characters long and contain only letters and numbers.";
    } elseif (!preg_match($validatePassword, $password)) {
        $error_message = "Password must be at least 8 characters long and include at least one letter, one number, and one special character.";
    } elseif ($password !== $confirmPassword) {
        $error_message = "Passwords do not match.";
    } else {
        // Proceed with signup API call
        $api_url = $baseAPI . "Users/signup";
        $data = json_encode([
            "email" => $email,
            "username" => $username,
            "password" => $password
        ]);

        $ch = curl_init($api_url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
        curl_setopt($ch, CURLOPT_HTTPHEADER, ["Content-Type: application/json"]);

        $response = curl_exec($ch);
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $curl_error = curl_error($ch);
        curl_close($ch);

        $result = json_decode($response, true);

        if ($http_code == 201 && isset($result['message'])) {
            header("Location: " . $baseURL . "pages/login.php?signup=success");
            exit;
        } else {
            $error_message = $curl_error ?: ($result['message'] ?? "Signup failed. Please try again.");
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
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <section>
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <div class="flex items-center mb-6 text-2xl font-semibold text-gray-900 dark:text-white">
        <img class="w-16 h-16 mr-2" src="<?= $baseURL; ?>images/Sentinel-logo.png" alt="logo">
        Sentinel
      </div>

      <div class="w-full bg-white rounded-lg shadow dark:border sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
        <div class="p-6 space-y-4 md:space-y-6 sm:p-8">
          <h1 class="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
            Create an account
          </h1>

          <?php if (!empty($error_message)) : ?>
            <p class="text-red-500 text-sm"><?= htmlspecialchars($error_message); ?></p>
          <?php endif; ?>

          <form class="space-y-4 md:space-y-6" method="POST">
            <!-- Email -->
            <div>
              <label for="email" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Email</label>
              <input type="email" name="email" id="email" required placeholder="name@example.com"
                value="<?= htmlspecialchars($email); ?>"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:text-white">
            </div>

            <!-- Username -->
            <div>
              <label for="username" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Username</label>
              <input type="text" name="username" id="username" required placeholder="Username"
                value="<?= htmlspecialchars($username); ?>"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:text-white">
            </div>

            <!-- Password -->
            <div>
              <label for="password" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Password</label>
              <input type="password" name="password" id="password" required placeholder="••••••••"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:text-white">
            </div>

            <!-- Confirm password -->
            <div>
              <label for="confirm-password" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Confirm password</label>
              <input type="password" name="confirm-password" id="confirm-password" required placeholder="••••••••"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:text-white">
            </div>

            <!-- Submit -->
            <button type="submit"
              class="w-full text-white bg-blue-600 hover:bg-blue-700 font-medium rounded-lg text-sm px-5 py-2.5 text-center">
              Create an account
            </button>

            <p class="text-sm font-light text-gray-500 dark:text-gray-400">
              Already have an account?
              <a href="<?= $baseURL ?>login" class="font-medium text-blue-600 hover:underline">Login here</a>
            </p>
          </form>
        </div>
      </div>
    </div>
  </section>

  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>

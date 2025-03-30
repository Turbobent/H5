<?php
session_start();
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");

// Redirect if not logged in
require_login();
?>

<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sentinel - Dashboard</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">

  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Main -->
  <section>
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <div
        class="flex flex-col items-center justify-center gap-4 bg-[#D3D3D3] h-[200px] w-[500px] rounded-lg p-6 shadow">

        <h2 class="text-xl font-bold text-gray-800">Welcome to your dashboard</h2>

      </div>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>

</body>

</html>
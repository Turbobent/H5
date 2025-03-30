<?php 
  include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
  include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");
?>
<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>About Sentinel – Our Mission, Team & Vision</title>
  <meta name="description"
    content="Learn more about Sentinel – who we are, what we stand for, and the team behind our mission. Discover our values, goals, and the story that drives us forward.">
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- main -->
  <section class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?>">
      <h2 class="<?= $sectionHeading ?>">About Us</h2>
      <p class="<?= $sectionParagraph ?>">
        At Sentinel, we believe that peace of mind starts at home. That’s why we design and develop advanced alarm
        security devices built to protect what matters most - your family, your property, and your peace of mind. Our
        technology is driven by reliability, simplicity, and innovation, giving homeowners smart solutions that are easy
        to install, intuitive to use, and tough on intruders. Backed by a passionate team of engineers and security
        experts, Sentinel is committed to making homes safer, smarter, and more secure - one device at a time.
      </p>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
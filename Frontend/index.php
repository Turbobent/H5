<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <?php include_once('templates/links.php') ?>
  <title>Sentinel</title>
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <section>
    <div class="<?= $defaultCenterAndFixedHeight ?>">

      <!-- Main -->
      <h1 class="font-bold">Hello World!</h1>

    </div>
  </section>
  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
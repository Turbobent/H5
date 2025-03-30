<?php 
  include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
  include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/tailwind-styling.php");
?>
<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Contact Sentinel – Get Support, Ask Questions</title>
  <meta name="description"
    content="Get in touch with the Sentinel team. Whether you have questions, need support, or want to collaborate — we're here to help. Reach out today.">
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- main -->
  <section class="<?= $defaultCenterAndFixedHeight ?>">
    <div class="<?= $sectionBox ?>">
      <h2 class="<?= $sectionHeading ?>">Contact Us</h2>
      <p class="<?= $sectionParagraph ?>">Got a technical issue?
        Want to send feedback about a beta feature? Need details about our Business plan? Let us know.</p>
      <form action="#" class="space-y-8">
        <div>
          <label class="<?= $formLabel ?>" for="email">Your email</label>
          <input type="email" id="email"
            class="<?= $formInput ?>"
            placeholder="name@mail.com" required>
        </div>
        <div>
          <label for="subject" class="<?= $formLabel ?>">Subject</label>
          <input type="text" id="subject"
            class="<?= $formInput ?>"
            placeholder="Let us know how we can help you" required>
        </div>
        <div class="sm:col-span-2">
          <label for="message" class="<?= $formLabel ?>">Your
            message</label>
          <textarea id="message" rows="6"
            class="<?= $formTextarea ?>"
            placeholder="Leave a comment..." required></textarea>
          <!-- Submit Btn -->
          <button type="submit"
            class="<?= $formButton ?>">Send
            message</button>
          </div>
      </form>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
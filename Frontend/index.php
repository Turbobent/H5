<!DOCTYPE html>
<html lang="en">

<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <?php include_once('templates/links.php') ?>
  <title>Sentinel - Advanced Home Security</title>
  <meta name="description" content="Sentinel is your all-in-one smart home security solution. Protect your home 24/7 with advanced alarm devices, real-time alerts, and effortless setup. Start securing your home today!">
</head>

<body class="<?= $defaultBackgroundColor ?>">
  <!-- Header -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/header.php"); ?>

  <!-- Hero Section -->
  <section class="bg-gradient-to-r from-teal-500 to-blue-600 text-white py-16">
    <div class="<?= $defaultCenterAndFixedHeight ?> text-center">
      <h1 class="text-5xl font-extrabold mb-4">Protect What Matters Most with Sentinel</h1>
      <p class="text-lg mb-6">Smart home security solutions for ultimate peace of mind, day or night.</p>
      <a href="<?= $baseURL ?>register-device"
        class="bg-blue-800 hover:bg-blue-900 text-white px-6 py-3 rounded-lg text-lg font-semibold transition duration-300">Get
        Started</a>
    </div>
  </section>

  <!-- Features Section -->
  <section class="py-20 bg-gray-50">
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <h2 class="text-3xl font-extrabold mb-8 text-center">Why Choose Sentinel?</h2>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
        <!-- Feature 1 -->
        <div class="bg-white p-6 rounded-lg shadow-lg">
          <h3 class="text-xl font-semibold mb-4">24/7 Monitoring</h3>
          <p class="text-gray-700">Our systems keep a watchful eye on your home around the clock, providing peace of
            mind no matter where you are.</p>
        </div>
        <!-- Feature 2 -->
        <div class="bg-white p-6 rounded-lg shadow-lg">
          <h3 class="text-xl font-semibold mb-4">Smart Alerts</h3>
          <p class="text-gray-700">Receive instant notifications on your phone if anything suspicious happens. Stay
            informed and in control at all times.</p>
        </div>
        <!-- Feature 3 -->
        <div class="bg-white p-6 rounded-lg shadow-lg">
          <h3 class="text-xl font-semibold mb-4">Easy Setup</h3>
          <p class="text-gray-700">Get up and running in minutes with our simple plug-and-play devices. No technical
            knowledge required.</p>
        </div>
      </div>
    </div>
  </section>

  <!-- Testimonials Section -->
  <section class="py-20 bg-blue-100">
    <div class="<?= $defaultCenterAndFixedHeight ?>">
      <h2 class="text-3xl font-extrabold mb-8 text-center">What Our Customers Are Saying</h2>
      <div class="flex flex-wrap justify-center gap-12">
        <div class="bg-white p-8 rounded-lg shadow-lg w-80">
          <p class="text-lg font-light mb-4">"Sentinel’s security system gave me peace of mind while on vacation. I knew
            my house was protected 24/7!"</p>
          <p class="font-semibold text-blue-600">Jane Doe</p>
          <p class="text-gray-500">Happy Customer</p>
        </div>
        <div class="bg-white p-8 rounded-lg shadow-lg w-80">
          <p class="text-lg font-light mb-4">"The installation was so easy, and the alerts are timely. I feel so much
            safer knowing Sentinel has my back."</p>
          <p class="font-semibold text-blue-600">John Smith</p>
          <p class="text-gray-500">Satisfied Client</p>
        </div>
      </div>
    </div>
  </section>

  <!-- Call to Action Section -->
  <section class="py-16 bg-gradient-to-r from-teal-600 to-blue-600 text-white">
    <div class="<?= $defaultCenterAndFixedHeight ?> text-center">
      <h2 class="text-3xl font-extrabold mb-4">Ready to Protect Your Home?</h2>
      <p class="text-lg mb-6">Take the first step in securing your home with Sentinel’s advanced home security devices.
      </p>
      <a href="<?= $baseURL ?>register-device"
        class="bg-blue-800 hover:bg-blue-900 text-white px-6 py-3 rounded-lg text-lg font-semibold transition duration-300">Get
        Started Now</a>
    </div>
  </section>

  <!-- Footer -->
  <?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/footer.php"); ?>
</body>

</html>
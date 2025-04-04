<!-- Include our links.php -->
<?php
if (session_status() === PHP_SESSION_NONE) {
  session_start();
}

include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/includes/auth.php");

// Handle logout
if ($_SERVER["REQUEST_METHOD"] == "POST" && isset($_POST['logout'])) {
  session_destroy();
  header("Location: " . $baseURL . "login");
  exit;
}
?>

<header class="bg-[#282828]">
  <div class="mx-auto flex h-[70px] max-w-screen-xl items-center gap-8 px-4 sm:px-6 lg:px-8">
    <a class="block text-teal-600" href="<?= $baseURL . (is_logged_in() ? 'dashboard' : 'index.php'); ?>">
      <span class="sr-only">Home</span>
      <img class="w-12 h-12 mr-2" src="<?= $baseURL; ?>images/Sentinel-logo.png" alt="logo">
    </a>

    <div class="flex flex-1 items-center justify-end md:justify-between">
      <nav aria-label="Global" class="hidden md:block">
        <!-- Hide links if not logged in -->
        <?php if (is_logged_in()) : ?>
        <ul class="flex items-center gap-6 text-sm">
          <!-- Dashboard -->
          <li>
            <a class="text-gray-500 transition hover:text-gray-500/75" href="<?= $baseURL ?>dashboard">Dashboard</a>
          </li>
          <!-- Edit Profile -->
          <li>
            <a class="text-gray-500 transition hover:text-gray-500/75" href="<?= $baseURL ?>edit-profile">Edit
              Profile</a>
          </li>
          <?php endif; ?>
        </ul>
      </nav>

      <?php if (!is_logged_in()) : ?>
      <!-- Login/Signup buttons -->
      <div class="flex items-center gap-4">
        <div class="sm:flex sm:gap-4">
          <a class="block rounded-md bg-teal-600 px-5 py-2.5 text-sm font-medium text-white transition hover:bg-teal-700"
            href="<?= $baseURL; ?>login">
            Login
          </a>
          <a class="hidden rounded-md bg-gray-100 px-5 py-2.5 text-sm font-medium text-teal-600 transition hover:text-teal-600/75 sm:block"
            href="<?= $baseURL; ?>signup">
            Signup
          </a>
        </div>
      </div>
      <?php else : ?>
      <!-- Logout button -->
      <form method="POST" action="<?= htmlspecialchars($_SERVER['PHP_SELF']); ?>">
        <button type="submit" name="logout"
          class="px-5 py-2.5 bg-red-600 text-white rounded-md text-sm font-medium hover:bg-red-700 transition">
          Logout
        </button>
      </form>
      <?php endif; ?>

      <button class="block rounded-sm bg-gray-100 p-2.5 text-gray-600 transition hover:text-gray-600/75 md:hidden">
        <span class="sr-only">Toggle menu</span>
        <svg xmlns="http://www.w3.org/2000/svg" class="size-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"
          stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16" />
        </svg>
      </button>
    </div>
  </div>
  </div>
</header>
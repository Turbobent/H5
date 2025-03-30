<!-- Include our links.php -->
<?php 
  include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php");
  
  // Block access to this page
  if (basename($_SERVER['PHP_SELF']) === basename(__FILE__)) {
    http_response_code(403);
    exit('Access denied.');
  }
?>

<footer class="bg-[#282828]">
  <div
    class="mx-auto flex flex-col md:flex-row h-[70px] max-w-screen-xl items-center justify-between gap-8 px-4 sm:px-6 lg:px-8">

    <!-- Copyright Text -->
    <div class="text-center md:text-left w-full md:w-auto">
      <span class="text-sm text-gray-500 dark:text-gray-400">
        © 2023 <a href="<?= $baseURL; ?>" class="hover:underline">Sentinel™</a>. All Rights Reserved.
      </span>
    </div>

    <!-- Footer Navigation -->
    <nav aria-label="Footer Navigation">
      <ul class="flex items-center gap-6 text-sm">
        <li>
          <a class="text-gray-500 transition hover:text-gray-500/75" href="<?= $baseURL; ?>about">About</a>
        </li>
        <li>
          <a class="text-gray-500 transition hover:text-gray-500/75"
            href="<?= $baseURL; ?>contact">Contact</a>
        </li>
      </ul>
    </nav>

  </div>
</footer>
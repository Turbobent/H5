<!-- Include our links.php -->
<?php include_once($_SERVER['DOCUMENT_ROOT'] . "/H5/Frontend/templates/links.php"); ?>

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
          <a class="text-gray-500 transition hover:text-gray-500/75" href="<?= $baseURL; ?>pages/about.php">About</a>
        </li>
        <li>
          <a class="text-gray-500 transition hover:text-gray-500/75"
            href="<?= $baseURL; ?>pages/contact.php">Contact</a>
        </li>
      </ul>
    </nav>

  </div>
</footer>
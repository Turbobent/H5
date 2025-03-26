<?php
if (session_status() === PHP_SESSION_NONE) {
  session_start();
}

function require_login() {
  global $baseURL;
  if (!isset($_SESSION['user_token'])) {
    header("Location: " . $baseURL . "pages/login.php");
    exit;
  }
}

function is_logged_in() {
  return isset($_SESSION['user_token']);
}
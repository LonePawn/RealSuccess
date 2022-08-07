<?php
	$server = "localhost";
	$user = "root";
	$pass = "";
	$db = "smart_kobo";						// remember to select the correct database and access details here

	$conn = new mysqli($server, $user, $pass, $db);

	if ($conn->connect_error)
	{
		die("Error connecting to dbase: ".$conn->connect_error);	//remember to remove the error details when done testing
	}
?>
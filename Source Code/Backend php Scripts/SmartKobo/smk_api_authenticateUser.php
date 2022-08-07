<?php
    include_once "../rss_OpenDb.php";
    include_once "../rss_functions.php";

    $email = isset($_POST['email']) ? mysqli_real_escape_string($conn, $_POST['email']) : "";
    $pwd = isset($_POST['pwd']) ? mysqli_real_escape_string($conn, $_POST['pwd']) : "";
    $generatedAuthId = "";
    $failReason = ""; 				// This will be used to log the reason for request failure should the request fail
    $requestResult = "failure";		// This will be used to log the status of the request upon response. There are only 2 values - 'success' or 'failure'

    if($email != "" && $pwd != "")
    {
    	$sql = "SELECT * FROM rss_account WHERE Email = '$email'";
    	$result = $conn->query($sql);
    	if($result->num_rows == 1)
    	{
    		$row = $result->fetch_assoc();
    		$dbPwd = $row['PWord'];
    		//$actId = $row['ActivationId'];

    		//if($actId == "")
    		//{
	    		if($pwd == $dbPwd)
	    		{
	    			$generatedAuthId = random_str(32);
	    			
	    			$sql = "UPDATE opn_account SET AuthId = '$generatedAuthId' WHERE Email = '$email'";
	    			$result2 = $conn->query($sql);
	    			if(!$result)
	    			{
	    				$generatedAuthId = "";	// clear the authId to avoid the script from returning an unset authId
	    				$failReason = "An unexpected error occured";
	    			}
	    			else
	    			{
	    				$requestResult = "success";
	    			}
	    		}
	    		else
	    		{
	    			$failReason = "Wrong password";
	    		}
	    	//}
	    	//else
	    	//{
	    	//	$failReason = "Account has not yet been activated";
	    	//}
    	}
    	else
    	{
    		$failReason = "User account does not exist";
    	}
    }
    else
    {
    	$failReason = "Invalid user email and/or password";
    }

	// construct the response array
	$response = array();
	$response["API"] = "SMK";
	$response["RequestResult"] = $requestResult;
	if($requestResult != "success") $response["failReason"] = $failReason;

	// construct the payload and attach it to the response
	$data = array();
	$data["AuthId"] = $generatedAuthId;
	$response["Data"] = $data;

	// encode the response array to json
	$jsonResponse = json_encode($response);

	// output the response
	echo $jsonResponse;
?>
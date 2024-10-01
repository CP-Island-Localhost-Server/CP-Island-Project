using System;
using System.Collections.Generic;
using Sfs2X.Logging;

namespace Sfs2X.Util
{
	public static class SFSErrorCodes
	{
		private static Dictionary<int, string> errorsByCode = new Dictionary<int, string>
		{
			{ 0, "Client API version is obsolete: {0}; required version: {1}" },
			{ 1, "Requested Zone {0} does not exist" },
			{ 2, "User name {0} is not recognized" },
			{ 3, "Wrong password for user {0}" },
			{ 4, "User {0} is banned" },
			{ 5, "Zone {0} is full" },
			{ 6, "User {0} is already logged in Zone {1}" },
			{ 7, "The server is full" },
			{ 8, "Zone {0} is currently inactive" },
			{ 9, "User name {0} contains bad words; filtered: {1}" },
			{ 10, "Guest users not allowed in Zone {0}" },
			{ 11, "IP address {0} is banned" },
			{ 12, "A Room with the same name already exists: {0}" },
			{ 13, "Requested Group is not available - Room: {0}; Group: {1}" },
			{ 14, "Bad Room name length -  Min: {0}; max: {1}; passed name length: {2}" },
			{ 15, "Room name contains bad words: {0}" },
			{ 16, "Zone is full; can't add Rooms anymore" },
			{ 17, "You have exceeded the number of Rooms that you can create per session: {0}" },
			{ 18, "Room creation failed, wrong parameter: {0}" },
			{ 19, "Room {0} already joined" },
			{ 20, "Room {0} is full" },
			{ 21, "Wrong password for Room {0}" },
			{ 22, "Requested Room does not exist" },
			{ 23, "Room {0} is locked" },
			{ 24, "Group {0} is already subscribed" },
			{ 25, "Group {0} does not exist" },
			{ 26, "Group {0} is not subscribed" },
			{ 27, "Group {0} does not exist" },
			{ 28, "{0}" },
			{ 29, "Room permission error; Room {0} cannot be renamed" },
			{ 30, "Room permission error; Room {0} cannot change password statee" },
			{ 31, "Room permission error; Room {0} cannot change capacity" },
			{ 32, "Switch user error; no player slots available in Room {0}" },
			{ 33, "Switch user error; no spectator slots available in Room {0}" },
			{ 34, "Switch user error; Room {0} is not a Game Room" },
			{ 35, "Switch user error; you are not joined in Room {0}" },
			{ 36, "Buddy Manager initialization error, could not load buddy list: {0}" },
			{ 37, "Buddy Manager error, your buddy list is full; size is {0}" },
			{ 38, "Buddy Manager error, was not able to block buddy {0} because offline" },
			{ 39, "Buddy Manager error, you are attempting to set too many Buddy Variables; limit is {0}" },
			{ 40, "Game {0} access denied, user does not match access criteria" },
			{ 41, "QuickJoin action failed: no matching Rooms were found" },
			{ 42, "Your previous invitation reply was invalid or arrived too late" }
		};

		public static void SetErrorMessage(int code, string message)
		{
			errorsByCode[code] = message;
		}

		public static string GetErrorMessage(int code, Logger log, params object[] args)
		{
			try
			{
				return string.Format(errorsByCode[code], args);
			}
			catch (Exception ex)
			{
				log.Error("Formatting error string failed with exception: " + ex.Message);
				return errorsByCode[code];
			}
		}
	}
}

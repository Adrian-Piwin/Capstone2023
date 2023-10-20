using UnityEngine;
using System.Threading.Tasks;
using Firebase.Storage;
using System;

public class FirebaseService
{
    private FirebaseStorage storage;

    // Start is called before the first frame update
    public FirebaseService()
    {
        storage = FirebaseStorage.DefaultInstance;
    }

    public async Task<byte[]> GetImageBytes(string path)
    {
        if (storage == null)
        {
            Debug.LogError("Firebase Storage is not initialized.");
            return null;
        }

        try
        {
            // Create a reference to the image file
            StorageReference reference = storage.GetReference(path);

            // Get the download URL for the image
            var urlTask = reference.GetDownloadUrlAsync();

            await urlTask;

            if (urlTask.IsCompleted)
            {
                string downloadUrl = urlTask.Result.ToString();

                // Download the image as a byte array
                var downloadTask = reference.GetBytesAsync(2048 * 2048);

                await downloadTask;

                if (downloadTask.IsCompleted)
                {
                    byte[] imageBytes = downloadTask.Result;
                    return imageBytes;
                }
                else
                {
                    Debug.LogError("Failed to download image: " + downloadTask.Exception);
                }
            }
            else
            {
                Debug.LogError("Failed to get download URL: " + urlTask.Exception);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error fetching image from Firebase Storage: " + e.Message);
        }

        return null;
    }
}
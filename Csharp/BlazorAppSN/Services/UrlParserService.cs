namespace BlazorAppSN.Services;

public class UrlParserService
{
    public string GetEmbedUrl(string url)
    {
        if (url.Contains("youtube.com") || url.Contains("youtu.be"))
        {
            return $"https://www.youtube.com/embed/{ExtractYouTubeVideoId(url)}";
        }
        else if (url.Contains("vimeo.com"))
        {
            return $"https://player.vimeo.com/video/{ExtractVimeoVideoId(url)}";
        }
        else if (url.Contains("dailymotion.com"))
        {
            return $"https://www.dailymotion.com/embed/video/{ExtractDailymotionVideoId(url)}";
        }
        else if (url.Contains("spotify.com"))
        {
            return $"https://open.spotify.com/embed/episode/{ExtractSpotifyPodcastId(url)}";
        }
        else if (url.Contains("soundcloud.com"))
        {
            return $"https://w.soundcloud.com/player/?url={Uri.EscapeDataString(url)}";
        }
        return url; 
    }

    public string ExtractYouTubeVideoId(string url)
    {
        var uri = new Uri(url);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        if (query.TryGetValue("v", out var videoId))
        {
            return videoId;
        }
        else if (uri.AbsolutePath.Contains("youtu.be"))
        {
            return uri.Segments.Last();
        }
        return string.Empty;
    }

    public string ExtractVimeoVideoId(string url)
    {
        var uri = new Uri(url);
        return uri.Segments.Last();
    }

    public string ExtractDailymotionVideoId(string url)
    {
        var uri = new Uri(url);
        return uri.Segments.Last();
    }

    public string ExtractSpotifyPodcastId(string url)
    {
        var uri = new Uri(url);
        return uri.Segments.Last();
    }

    public bool IsDirectVideoUrl(string url)
    {
        return url.EndsWith(".mp4") || url.EndsWith(".webm") || url.EndsWith(".ogg");
    }

    public bool IsDirectAudioUrl(string url)
    {
        return url.EndsWith(".mp3") || url.EndsWith(".wav") || url.EndsWith(".ogg");
    }

    public bool IsDirectImageUrl(string url)
    {
        return url.EndsWith(".jpg") || url.EndsWith(".jpeg") || url.EndsWith(".png") || url.EndsWith(".gif");
    }

    public bool IsRecognizedUrl(string url)
    {
        return url.Contains("youtube.com") || url.Contains("youtu.be") || url.Contains("vimeo.com") ||
               url.Contains("dailymotion.com") || url.Contains("spotify.com") || url.Contains("soundcloud.com") ||
               IsDirectVideoUrl(url) || IsDirectAudioUrl(url) || IsDirectImageUrl(url);
    }

    public bool IsImgurUrl(string url)
    {
        return url.Contains("imgur.com");
    }

    public bool IsFlickrUrl(string url)
    {
        return url.Contains("flickr.com");
    }

    public bool IsGooglePhotosUrl(string url)
    {
        return url.Contains("photos.google.com");
    }

    public string GetDirectImageUrl(string url)
    {
        if (IsImgurUrl(url))
        {
            return url;
        }
        else if (IsFlickrUrl(url))
        {
            return url;
        }
        else if (IsGooglePhotosUrl(url))
        {
            return GetGooglePhotosDirectUrl(url);
        }
        return url; 
    }

    public string GetGooglePhotosDirectUrl(string url)
    {
        
        return url.Replace("photos.google.com", "lh3.googleusercontent.com").Replace("/photo/", "/");
    }
}

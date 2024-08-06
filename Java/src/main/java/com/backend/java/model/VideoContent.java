package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeName;
import jakarta.persistence.Entity;
import jakarta.persistence.Table;

@Entity
@Table(name = "video_content")
@JsonTypeName("video")
public class VideoContent extends Content {

    @JsonProperty("videoUrl")
    private String videoUrl;
    @JsonProperty("duration")
    private int duration;

    // Constructors, getters, and setters
    public VideoContent() {}

    public VideoContent(String videoUrl, int duration) {
        this.videoUrl = videoUrl;
        this.duration = duration;
    }

    public String getVideoUrl() {
        return videoUrl;
    }

    public void setVideoUrl(String videoUrl) {
        this.videoUrl = videoUrl;
    }

    public int getDuration() {
        return duration;
    }

    public void setDuration(int duration) {
        this.duration = duration;
    }

    @Override
    public String toString() {
        return "VideoContent{" +
                "videoUrl='" + videoUrl + '\'' +
                '}';
    }
}

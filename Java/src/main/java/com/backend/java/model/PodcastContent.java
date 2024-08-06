package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeName;
import jakarta.persistence.Entity;
import jakarta.persistence.Table;

@Entity
@Table(name = "podcast_content")
@JsonTypeName("podcast")
public class PodcastContent extends Content {
    @JsonProperty("audioUrl")
    private String audioUrl;
    @JsonProperty("duration")
    private int duration;

    // Constructors, getters, and setters
    public PodcastContent() {}

    public PodcastContent(String audioUrl, int duration) {
        this.audioUrl = audioUrl;
        this.duration = duration;
    }

    public String getAudioUrl() {
        return audioUrl;
    }

    public void setAudioUrl(String audioUrl) {
        this.audioUrl = audioUrl;
    }

    public int getDuration() {
        return duration;
    }

    public void setDuration(int duration) {
        this.duration = duration;
    }

    @Override
    public String toString() {
        return "PodcastContent{" +
                "audioUrl='" + audioUrl + '\'' +
                '}';
    }
}

package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeName;
import jakarta.persistence.Entity;
import jakarta.persistence.Table;

@Entity
@Table(name = "photo_content")
@JsonTypeName("photo")
public class PhotoContent extends Content {

    @JsonProperty("imageUrl")
    private String imageUrl;

    // Constructors, getters, and setters
    public PhotoContent() {}

    public PhotoContent(String imageUrl) {
        this.imageUrl = imageUrl;
    }

    public String getImageUrl() {
        return imageUrl;
    }

    public void setImageUrl(String imageUrl) {
        this.imageUrl = imageUrl;
    }

    @Override
    public String toString() {
        return "PhotoContent{" +
                "imageUrl='" + imageUrl + '\'' +
                '}';
    }
}

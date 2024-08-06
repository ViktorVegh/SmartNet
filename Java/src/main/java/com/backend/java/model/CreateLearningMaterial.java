package com.backend.java.model;

import com.fasterxml.jackson.databind.annotation.JsonDeserialize;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class CreateLearningMaterial {
    private String headline;
    private String description;
    private boolean membersOnly;
    private Long userId;
    private Date createdAt;
    private Date updatedAt;

    @JsonDeserialize(contentAs = Content.class)
    private List<Content> contents;


    public CreateLearningMaterial() {
        this.contents = new ArrayList<>();
    }

    public CreateLearningMaterial(String headline, String description, Boolean membersOnly, Long userId, Date createdAt, Date updatedAt, List<Content> contents) {
        this.headline = headline;
        this.description = description;
        this.membersOnly = membersOnly;
        this.userId = userId;
        this.createdAt = createdAt;
        this.updatedAt = updatedAt;
        this.contents = contents != null ? contents : new ArrayList<>();
    }


    public String getHeadline() {
        return headline;
    }

    public void setHeadline(String headline) {
        this.headline = headline;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Long getUserId() {
        return userId;
    }

    public void setUserId(Long userId) {
        this.userId = userId;
    }

    public Date getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(Date createdAt) {
        this.createdAt = createdAt;
    }

    public Date getUpdatedAt() {
        return updatedAt;
    }

    public void setUpdatedAt(Date updatedAt) {
        this.updatedAt = updatedAt;
    }

    public List<Content> getContents() {
        return contents;
    }

    public void setContents(List<Content> contents) {
        this.contents = contents;
    }

    @Override
    public String toString() {
        return "CreateLearningMaterial{" +
                "headline='" + headline + '\'' +
                ", description='" + description + '\'' +
                ", userId=" + userId +
                ", createdAt=" + createdAt +
                ", updatedAt=" + updatedAt +
                ", contents=" + contents +
                '}';
    }

    public boolean isMembersOnly() {
        return membersOnly;
    }

    public void setMembersOnly(boolean membersOnly) {
        this.membersOnly = membersOnly;
    }
}

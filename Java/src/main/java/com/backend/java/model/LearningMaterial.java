package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import com.fasterxml.jackson.annotation.JsonProperty;
import jakarta.persistence.*;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

@Entity
@Table(name = "learning_materials")
public class LearningMaterial {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String headline;
    private String description;
    private boolean membersOnly;
    private Date createdAt;
    private Date updatedAt;

    @ManyToOne
    @JoinColumn(name = "user_id")
    @JsonIgnore
    private User user;

    @Transient
    @JsonProperty("userId")
    private Long userId;

    @OneToMany(mappedBy = "learningMaterial", cascade = CascadeType.ALL, orphanRemoval = true)
    @JsonManagedReference
    private List<Content> contents = new ArrayList<>();


    public LearningMaterial() {}

    public LearningMaterial(String headline, String description, Boolean membersOnly, User user, Date createdAt, Date updatedAt, List<Content> contents) {
        this.headline = headline;
        this.description = description;
        this.membersOnly = membersOnly;
        this.user = user;
        this.createdAt = createdAt;
        this.updatedAt = updatedAt;
        this.contents = contents;
        this.userId = user != null ? user.getId() : null;
    }


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
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

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
        this.userId = user != null ? user.getId() : null;
    }

    public List<Content> getContents() {
        return contents;
    }

    public void setContents(List<Content> contents) {
        this.contents = contents;
    }

    @JsonProperty("userId")
    public Long getUserId() {
        return userId;
    }

    @PostLoad
    @PostPersist
    @PostUpdate
    private void updateUserId() {
        this.userId = user != null ? user.getId() : null;
    }

    @Override
    public String toString() {
        return "LearningMaterial{" +
                "id=" + id +
                ", headline='" + headline + '\'' +
                ", description='" + description + '\'' +
                ", userId=" + userId +
                ", createdAt='" + createdAt + '\'' +
                ", updatedAt='" + updatedAt + '\'' +
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

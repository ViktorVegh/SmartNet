package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import jakarta.persistence.*;

@JsonTypeInfo(
        use = JsonTypeInfo.Id.NAME,
        include = JsonTypeInfo.As.PROPERTY,
        property = "type"
)
@JsonSubTypes({
        @JsonSubTypes.Type(value = PhotoContent.class, name = "photo"),
        @JsonSubTypes.Type(value = PodcastContent.class, name = "podcast"),
        @JsonSubTypes.Type(value = TextContent.class, name = "text"),
        @JsonSubTypes.Type(value = VideoContent.class, name = "video")
})
@Entity
@Inheritance(strategy = InheritanceType.JOINED)
@Table(name = "content")
public abstract class Content {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "learning_material_id", nullable = false)
    @JsonBackReference
    private LearningMaterial learningMaterial;

    // Getters and setters
    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public LearningMaterial getLearningMaterial() {
        return learningMaterial;
    }

    public void setLearningMaterial(LearningMaterial learningMaterial) {
        this.learningMaterial = learningMaterial;
    }
}

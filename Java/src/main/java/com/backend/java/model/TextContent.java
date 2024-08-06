package com.backend.java.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonTypeName;
import jakarta.persistence.Entity;
import jakarta.persistence.Table;

@Entity
@Table(name = "text_content")
@JsonTypeName("text")
public class TextContent extends Content {
    @JsonProperty("body")
    private String body;

    // Constructors, getters, and setters
    public TextContent() {}

    public TextContent(String body) {
        this.body = body;
    }

    public String getBody() {
        return body;
    }

    public void setBody(String body) {
        this.body = body;
    }

    @Override
    public String toString() {
        return "TextContent{" +
                "body='" + body + '\'' +
                '}';
    }
}

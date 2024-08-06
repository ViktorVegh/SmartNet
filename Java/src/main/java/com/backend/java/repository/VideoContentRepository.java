package com.backend.java.repository;

import com.backend.java.model.VideoContent;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface VideoContentRepository extends ContentRepository<VideoContent> {

}

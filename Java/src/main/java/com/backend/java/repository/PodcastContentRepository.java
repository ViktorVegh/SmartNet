package com.backend.java.repository;

import com.backend.java.model.PodcastContent;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface PodcastContentRepository extends ContentRepository<PodcastContent> {
}

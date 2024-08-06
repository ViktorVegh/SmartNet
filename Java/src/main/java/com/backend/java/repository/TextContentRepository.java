package com.backend.java.repository;

import com.backend.java.model.TextContent;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface TextContentRepository extends ContentRepository<TextContent> {

}

package com.backend.java.repository;

import com.backend.java.model.Content;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.repository.NoRepositoryBean;

@NoRepositoryBean
public interface ContentRepository<T extends Content> extends JpaRepository<T, Long> {
}

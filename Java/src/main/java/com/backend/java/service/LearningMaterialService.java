package com.backend.java.service;

import com.backend.java.iservice.ILearningMaterialService;
import com.backend.java.model.*;
import com.backend.java.model.DTOs.CreateLearningMaterial;
import com.backend.java.repository.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Service
public class LearningMaterialService implements ILearningMaterialService {

    @Autowired
    private LearningMaterialRepository learningMaterialRepository;

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private PhotoContentRepository photoContentRepository;

    @Autowired
    private PodcastContentRepository podcastContentRepository;

    @Autowired
    private TextContentRepository textContentRepository;

    @Autowired
    private VideoContentRepository videoContentRepository;

    @Transactional
    public LearningMaterial addLearningMaterial(CreateLearningMaterial dto) {
        User user = userRepository.findById(dto.getUserId())
                .orElseThrow(() -> new RuntimeException("User not found"));

        LearningMaterial learningMaterial = new LearningMaterial(
                dto.getHeadline(),
                dto.getDescription(),
                user,
                dto.getCreatedAt(),
                dto.getUpdatedAt(),
                new ArrayList<>()
        );

        learningMaterial = learningMaterialRepository.save(learningMaterial);

        saveContents(dto.getContents(), learningMaterial);

        return learningMaterialRepository.findById(learningMaterial.getId())
                .orElseThrow(() -> new RuntimeException("LearningMaterial not found"));
    }

    private void saveContents(List<Content> contents, LearningMaterial learningMaterial) {
        if (contents != null) {
            for (Content content : contents) {
                content.setLearningMaterial(learningMaterial);
                if (content instanceof PhotoContent) {
                    photoContentRepository.save((PhotoContent) content);
                } else if (content instanceof PodcastContent) {
                    podcastContentRepository.save((PodcastContent) content);
                } else if (content instanceof TextContent) {
                    textContentRepository.save((TextContent) content);
                } else if (content instanceof VideoContent) {
                    videoContentRepository.save((VideoContent) content);
                }
                learningMaterial.getContents().add(content);
            }
        }
    }

    public Optional<LearningMaterial> getLearningMaterialById(Long id) {
        return learningMaterialRepository.findById(id);
    }

    public List<LearningMaterial> getAllLearningMaterials() {
        return learningMaterialRepository.findAll();
    }

    @Transactional
    public LearningMaterial updateLearningMaterial(Long id, CreateLearningMaterial dto) {
        LearningMaterial existingMaterial = learningMaterialRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Learning Material not found"));

        existingMaterial.setHeadline(dto.getHeadline());
        existingMaterial.setDescription(dto.getDescription());
        existingMaterial.setUpdatedAt(new java.util.Date());

        existingMaterial.getContents().clear();
        saveContents(dto.getContents(), existingMaterial);

        return learningMaterialRepository.save(existingMaterial);
    }

    @Transactional
    public void deleteLearningMaterial(Long id) {
        if (!learningMaterialRepository.existsById(id)) {
            throw new RuntimeException("Learning Material not found");
        }
        learningMaterialRepository.deleteById(id);
    }
}

package com.backend.java.iservice;

import com.backend.java.model.DTOs.CreateLearningMaterial;
import com.backend.java.model.LearningMaterial;

import java.util.List;
import java.util.Optional;

public interface ILearningMaterialService {

    LearningMaterial addLearningMaterial(CreateLearningMaterial dto);
    Optional<LearningMaterial> getLearningMaterialById(Long id);
    List<LearningMaterial> getAllLearningMaterials();
    LearningMaterial updateLearningMaterial(Long id, CreateLearningMaterial dto);
    void deleteLearningMaterial(Long id);

}

package com.backend.java.controller;

import com.backend.java.iservice.ILearningMaterialService;
import com.backend.java.model.DTOs.CreateLearningMaterial;
import com.backend.java.model.LearningMaterial;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/learning-materials")
public class LearningMaterialController {

    @Autowired
    private ILearningMaterialService learningMaterialService;

    @PostMapping
    public ResponseEntity<LearningMaterial> addLearningMaterial(@RequestBody CreateLearningMaterial dto) {
        try {
            LearningMaterial createdLearningMaterial = learningMaterialService.addLearningMaterial(dto);
            return ResponseEntity.ok(createdLearningMaterial);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<LearningMaterial> getLearningMaterialById(@PathVariable Long id) {
        return learningMaterialService.getLearningMaterialById(id)
                .map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.notFound().build());
    }

    @GetMapping
    public ResponseEntity<List<LearningMaterial>> getAllLearningMaterials() {
        List<LearningMaterial> learningMaterials = learningMaterialService.getAllLearningMaterials();
        return ResponseEntity.ok(learningMaterials);
    }

    @PutMapping("/{id}")
    public ResponseEntity<LearningMaterial> updateLearningMaterial(@PathVariable Long id, @RequestBody CreateLearningMaterial dto) {
        LearningMaterial updatedLearningMaterial = learningMaterialService.updateLearningMaterial(id, dto);
        return ResponseEntity.ok(updatedLearningMaterial);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteLearningMaterial(@PathVariable Long id) {
        learningMaterialService.deleteLearningMaterial(id);
        return ResponseEntity.noContent().build();
    }
}

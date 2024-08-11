package com.backend.java;

import com.backend.java.model.DTOs.CreateLearningMaterial;
import com.backend.java.model.PhotoContent;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

public class DeserializationTest {

    @Test
    public void testDeserializeLearningMaterial() throws Exception {
        String json = "{ \"headline\": \"Sample Learning Material\", \"description\": \"This is a sample description for the learning material.\", \"membersOnly\": true, \"userId\": 1, \"createdAt\": \"2024-08-04T17:39:49.423056Z\", \"updatedAt\": \"2024-08-04T17:39:49.423056Z\", \"contents\": [ { \"type\": \"photo\", \"imageUrl\": \"http://example.com/image1.jpg\" } ] }";

        ObjectMapper mapper = new ObjectMapper();
        CreateLearningMaterial dto = mapper.readValue(json, CreateLearningMaterial.class);

        assertNotNull(dto);
        assertEquals("Sample Learning Material", dto.getHeadline());
        assertEquals(1, dto.getUserId());
        assertEquals(1, dto.getContents().size());
        assertTrue(dto.getContents().get(0) instanceof PhotoContent);
        assertEquals("http://example.com/image1.jpg", ((PhotoContent) dto.getContents().get(0)).getImageUrl());
    }
}

package com.backend.java.service;

import com.backend.java.iservice.IUserService;
import com.backend.java.model.DTOs.UserDto;
import com.backend.java.model.LearningMaterial;
import com.backend.java.model.User;
import com.backend.java.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import java.util.Optional;

@Service
public class UserService implements IUserService {

    @Autowired
    private UserRepository userRepository;

    public Optional<User> findByUsername(String username) {
        return userRepository.findByUsername(username);
    }

    public Optional<User> findById(Long id) {
        return userRepository.findById(id);
    }

    public Optional<User> findByEmail(String email) {return userRepository.findByEmail(email);}

    public Optional<UserDto> findUserDtoById(Long id) {
        Optional<User> user = userRepository.findById(id);
        return user.map(this::convertToDto);
    }

    private UserDto convertToDto(User user) {
        UserDto dto = new UserDto();
        dto.setId(user.getId());
        dto.setUsername(user.getUsername());
        dto.setProfilePicture(user.getProfilePicture());
        return dto;
    }

}

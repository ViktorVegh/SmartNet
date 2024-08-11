package com.backend.java.iservice;

import com.backend.java.model.DTOs.UserDto;
import com.backend.java.model.User;

import java.util.Optional;

public interface IUserService {

    Optional<User> findByUsername(String username);
    Optional<User> findById(Long id);
    Optional<User> findByEmail(String email);
    Optional<UserDto> findUserDtoById(Long id);

}

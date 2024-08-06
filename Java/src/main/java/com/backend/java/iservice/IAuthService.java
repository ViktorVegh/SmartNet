package com.backend.java.iservice;

import com.backend.java.model.User;

public interface IAuthService {

    User registerUser(User user);
    boolean validatePassword(String rawPassword, String encodedPassword);

}


package com.backend.java;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import static org.springframework.security.config.Customizer.withDefaults;

@Configuration
public class SecurityConfig {

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        http.csrf(csrf -> csrf.disable())
                .authorizeHttpRequests(authorize -> authorize
                        .requestMatchers("/api/learning-materials", "/api/learning-materials/{id}").permitAll() // Allow unauthenticated access to GET all learning materials and by ID
                        .requestMatchers("/api/learning-materials/**").authenticated() // Require authentication for other endpoints in learning-materials
                        .anyRequest().permitAll()) // Allow unauthenticated access to other requests
                .httpBasic(withDefaults()); // or any other authentication method

        return http.build();
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}

package com.backend.java.service;

import com.backend.java.iservice.INotificationService;
import com.backend.java.model.DTOs.NotificationDto;
import com.backend.java.model.DTOs.UserDto;
import com.backend.java.model.Notification;
import com.backend.java.model.User;
import com.backend.java.repository.NotificationRepository;
import com.backend.java.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
public class NotificationService implements INotificationService {
    @Autowired
    private NotificationRepository notificationRepository;

    @Autowired
    private UserRepository userRepository;

    public void saveNotification(Long userId, String message) {
        User user = userRepository.findById(userId).orElseThrow(() -> new IllegalArgumentException("User not found"));

        Notification notification = new Notification();
        notification.setUser(user);
        notification.setMessage(message);
        notification.setDate(LocalDateTime.now());

        notificationRepository.save(notification);
    }

    @Override
    public List<NotificationDto> getNotifications(Long userId) {
        List<Notification> notifications = notificationRepository.findByUserId(userId);
        return notifications.stream()
                .map(this::convertToDTO)
                .collect(Collectors.toList());
    }

    private NotificationDto convertToDTO(Notification notification) {
        NotificationDto dto = new NotificationDto();
        dto.setId(notification.getId());
        dto.setMessage(notification.getMessage());
        dto.setDate(notification.getDate());
        dto.setUser(convertUserToDTO(notification.getUser()));
        return dto;
    }

    private UserDto convertUserToDTO(User user) {
        UserDto dto = new UserDto();
        dto.setId(user.getId());
        dto.setUsername(user.getUsername());
        dto.setProfilePicture(user.getProfilePicture());
        return dto;
    }
}

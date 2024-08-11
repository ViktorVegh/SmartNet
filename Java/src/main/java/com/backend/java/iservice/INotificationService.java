package com.backend.java.iservice;

import com.backend.java.model.DTOs.NotificationDto;
import com.backend.java.model.Notification;

import java.util.List;

public interface INotificationService {
    void saveNotification(Long userId, String message);
    List<NotificationDto> getNotifications(Long userId);
}

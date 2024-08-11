package com.backend.java.service;

import com.backend.java.iservice.ISubscriptionService;
import com.backend.java.model.Subscription;
import com.backend.java.model.User;
import com.backend.java.repository.SubscriptionRepository;
import com.backend.java.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;

@Service
public class SubscriptionService implements ISubscriptionService {
    @Autowired
    private SubscriptionRepository subscriptionRepository;

    @Autowired
    private UserRepository userRepository;

    public void saveSubscription(Long subscriberId, Long subscribedToId) {
        User subscriber = userRepository.findById(subscriberId)
                .orElseThrow(() -> new IllegalArgumentException("Subscriber not found"));
        User subscribedTo = userRepository.findById(subscribedToId)
                .orElseThrow(() -> new IllegalArgumentException("Subscribed to user not found"));

        Subscription subscription = new Subscription();
        subscription.setSubscriber(subscriber);
        subscription.setSubscribedTo(subscribedTo);
        subscription.setSubscribedAt(LocalDateTime.now());
        subscriptionRepository.save(subscription);
    }

    public void removeSubscription(Long subscriberId, Long subscribedToId) {
        User subscriber = userRepository.findById(subscriberId)
                .orElseThrow(() -> new IllegalArgumentException("Subscriber not found"));
        User subscribedTo = userRepository.findById(subscribedToId)
                .orElseThrow(() -> new IllegalArgumentException("Subscribed to user not found"));

        Subscription subscription = subscriptionRepository.findBySubscriberAndSubscribedTo(subscriber, subscribedTo);
        if (subscription != null) {
            subscriptionRepository.delete(subscription);
        }
    }

    public List<Subscription> getSubscribers(Long subscribedToId) {
        User subscribedTo = userRepository.findById(subscribedToId)
                .orElseThrow(() -> new IllegalArgumentException("Subscribed to user not found"));
        return subscriptionRepository.findBySubscribedTo(subscribedTo);
    }
}

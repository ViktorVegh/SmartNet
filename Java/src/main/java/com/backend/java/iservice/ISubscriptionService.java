package com.backend.java.iservice;

import com.backend.java.model.Subscription;

import java.util.List;

public interface ISubscriptionService {
    void saveSubscription(Long subscriberId, Long subscribedToId);
    void removeSubscription(Long subscriberId, Long subscribedToId);
    List<Subscription> getSubscribers(Long subscribedToId);
}

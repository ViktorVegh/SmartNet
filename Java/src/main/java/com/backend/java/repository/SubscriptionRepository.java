package com.backend.java.repository;

import com.backend.java.model.Subscription;
import com.backend.java.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface SubscriptionRepository extends JpaRepository<Subscription, Long> {
    List<Subscription> findBySubscribedTo(User subscribedTo);
    Subscription findBySubscriberAndSubscribedTo(User subscriber, User subscribedTo);
}

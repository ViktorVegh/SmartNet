package com.backend.java.controller;

import com.backend.java.iservice.ISubscriptionService;
import com.backend.java.model.Subscription;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/subscriptions")
public class SubscriptionController {
    @Autowired
    private ISubscriptionService subscriptionService;

    @PostMapping("/subscribe")
    public ResponseEntity<Void> subscribe(@RequestParam Long subscriberId, @RequestParam Long subscribedToId) {
        subscriptionService.saveSubscription(subscriberId, subscribedToId);
        return ResponseEntity.ok().build();
    }

    @PostMapping("/unsubscribe")
    public ResponseEntity<Void> unsubscribe(@RequestParam Long subscriberId, @RequestParam Long subscribedToId) {
        subscriptionService.removeSubscription(subscriberId, subscribedToId);
        return ResponseEntity.ok().build();
    }

    @GetMapping("/subscribers/{userId}")
    public ResponseEntity<List<Subscription>> getSubscribers(@PathVariable Long userId) {
        List<Subscription> subscribers = subscriptionService.getSubscribers(userId);
        return ResponseEntity.ok(subscribers);
    }
}

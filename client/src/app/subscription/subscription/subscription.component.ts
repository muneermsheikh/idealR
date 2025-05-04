import { Component, EnvironmentInjector, OnInit } from '@angular/core';
import { SubscriptionService } from 'src/app/_services/subscription/subscription.service';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.css']
})
export class SubscriptionComponent implements OnInit {

  subscription: any;
  additionalDays: number = 14;
  errorMessage: string | null = null;
  
  constructor(private subscriptionService: SubscriptionService) {}

  ngOnInit(): void {
    this.loadSubscriptionStatus();
  }

  loadSubscriptionStatus(): void {
    this.subscriptionService.getSubscriptionStatus().subscribe({
      next: (data) => {
        this.subscription = data;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load subscription status.';
        console.error(err);
      }
    });
  }

  extendTrial(): void {
    
    this.subscriptionService.extendTrial(this.additionalDays).subscribe({
      next: () => {
        this.loadSubscriptionStatus();
        alert('Trial extended successfully!');
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to extend trial.';
      }
    });
  }

  // Optional: Initiate Stripe payment
  initiatePayment(): void {
    // Integrate with Stripe.js or redirect to Stripe Checkout
    // On success, call convertToPaid(transactionId)
  }
  
}

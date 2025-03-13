import { Injectable } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AccountService } from '../account.service';
import { map, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  user?: any;

  constructor(private accountService: AccountService) {
    this.user == localStorage.getItem('user');
    if(!this.user) this.accountService.currentUser$.pipe(map(user => {if(user) {this.user = user}}));

  }

  // Set a value in local storage
  setItem(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  loggedInUserhasVisaEditRole(): boolean {
    if(!this.user) return false;
    return this.user.roles.includes('visaEdit');
  }

  // Get a value from local storage
  getLoggedInUserRoles(): string[] {
    if(!this.user) return [];
    return this.user.roles;
  }

  getLoggedInUserEmploer(): string {
    return this.user?.employer ?? "";
  }
  // Remove a value from local storage
  removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  // Clear all items from local storage
  clear(): void {
    localStorage.clear();
  }
}

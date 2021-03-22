import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  isAuthenticated: boolean = false;

  constructor(private http: HttpClient, private accountService: AccountService) { }

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe(user => {
      this.isAuthenticated = !!user;
    })
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  // getUsers() {
  //   this.http.get('http://localhost:5000/api/users').subscribe(users => this.users = users);
  // }

  cancelRegisterMode(mode: boolean) {
    this.registerMode = mode;
  }

}

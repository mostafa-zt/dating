import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() userFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      gender: new FormControl(''),
      username: new FormControl('', Validators.required),
      knownAs: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    })
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true }
    }
  }

  register() {
    debugger;
    console.log(this.registerForm.value);
    this.accountService.register(this.registerForm.value).subscribe(response => {
      console.log(response);
      this.router.navigateByUrl('/members');
      // this.cancel();
    }, error => {
      debugger;
      console.log(error);
      this.validationErrors = error;
    })
  }

  cancel() {
    console.log('cancelled!');
    this.cancelRegister.emit(false);
  }

}

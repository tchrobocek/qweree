import { Component, OnInit } from '@angular/core';
import {Guid} from '../../../services/Guid';

@Component({
  selector: 'app-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent implements OnInit {

  public id = '';
  public username = '';
  public password = '';
  public password2 = '';
  public email = '';
  public roles: string[] = [];

  constructor() { }

  ngOnInit(): void {
    this.id = Guid.newGuid();
  }

}

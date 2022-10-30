import { Component, Input, OnInit, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {User} from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit
{
  //..We receive input from user_management_component to roles_modal_component
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: any[];
  
  //..we need to inject the BsModalRef and access it
  //..in the template
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void 
  {
    
  }
  
  updateRoles()
  {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}

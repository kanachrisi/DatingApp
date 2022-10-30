import { Component, OnInit } from '@angular/core';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {User} from 'src/app/_models/user';
import { AdminService } from './../../services/admin.service';
import { RolesModalComponent } from './../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;
  
  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void 
  {
    this.getUsersWithRoles();
  }
  
  getUsersWithRoles()
  {
    this.adminService.getUsersWithRoles().subscribe(users =>
    {
      
      this.users = users;
    })
  }
  
  openRolesModal(user: User)
  {
    
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    
    this.bsModalRef.content.updateSelectedRoles.subscribe(values =>
    {
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate)
      {
        this.adminService.updateUserRoles(user.userName, rolesToUpdate.roles).subscribe(() =>
        {
          user.roles = [...rolesToUpdate.roles];
        })
      }
    });
    
  }
  
  private getRolesArray(user: User)
  {
    const roles = [];
    const userRoles = user.roles;
    
    //..the roles that would be displayed in modal checkboxes
    const availableRoles: any[] = [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'}
    ]
    
    //..we need to loop over available roles and see if they match
    //..with user roles. If they match we check them.
    
    availableRoles.forEach(role =>
    {
      let isMatch = false;
      for (const userRole of userRoles)
      {
        if (role.name === userRole)
        {
          role.checked = true;
          isMatch = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch)
      {
        role.checked = false;
        roles.push(role);
      }
    })
    return roles;
  }

}

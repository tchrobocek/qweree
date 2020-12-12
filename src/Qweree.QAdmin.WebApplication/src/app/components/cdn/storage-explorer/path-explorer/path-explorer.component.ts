import {Component, Input, OnInit} from '@angular/core';
import {ExplorerDirectory, ExplorerFile} from '../../../../model/cdn/ExplorerObject';
import {CdnAdapterService} from '../../../../services/cdn/cdn-adapter.service';

@Component({
  selector: 'app-path-explorer',
  templateUrl: './path-explorer.component.html',
  styleUrls: ['./path-explorer.component.scss']
})
export class PathExplorerComponent implements OnInit {

  @Input() public path: string;
  public directories: ExplorerDirectory[];
  public files: ExplorerFile[];

  constructor(
    private cdnAdapter: CdnAdapterService,
  ) {
    this.directories = [];
    this.files = [];
  }

  ngOnInit(): void {
    this.cdnAdapter.explore(this.path)
      .subscribe(objects => {
        objects.forEach(o => {
          const dir = (o as ExplorerDirectory);
          if (dir.totalCount) {
            this.directories.push(dir);
          }
          const file = (o as ExplorerFile);
          if (file.id) {
            this.files.push(file);
          }
        });

        console.log(objects);
        console.log(this.directories);
        console.log(this.files);
      });
  }
}

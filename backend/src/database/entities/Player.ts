import { Entity, Column, PrimaryColumn } from 'typeorm';

@Entity()
export class Player {
  @PrimaryColumn({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  player_id!: string;

  @Column({
    type: 'varchar',
    nullable: true,
    unique: true,
  })
  username!: string;

  @Column({ type: 'int', default: 0 })
  gold: number = 0;
}

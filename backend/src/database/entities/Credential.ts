import { Entity, Column, OneToOne, JoinColumn, PrimaryColumn } from 'typeorm';
import { Player } from './Player';

@Entity()
export class Credential {
  @PrimaryColumn({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  id!: string;

  @Column({
    type: 'varchar',
    nullable: true,
  })
  email!: string;

  @Column({
    type: 'varchar',
    nullable: true,
  })
  password!: string;

  @OneToOne(() => Player)
  @JoinColumn({ name: 'player_id' })
  player!: Player;
}
